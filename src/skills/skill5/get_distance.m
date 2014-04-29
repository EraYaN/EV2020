function [ diff ] = get_distance( n, L, tol)
    %initialize differences matrix (3d);
    diff = zeros(n,n,n);
    % Parralel loop
    parfor j1=1:n;
        for j2=1:n;
            for j3=1:n;
                % get seperate distances
                a1=abs(L(1,j1) - L(2,j2));
                a2=abs(L(2,j2) - L(3,j3));
                a3=abs(L(1,j1) - L(3,j3));
                % fill difference matrix
                diff(j1,j2,j3) = max([a1 a2 a3]);
            end
        end
    end
    disp('Finding minimum');
    toc
    %find lowest difference between the lines
    [value, index] = min(diff(:));
    if(value<tol)
        % convert index to matrix indices
        [p,q,s] = ind2sub(size(diff), index);    
        % take the average of the two lengths    
        length = (L(1,p)+L(2,q)+L(3,s))/3;
        % print resul
        fprintf('Solution: %f @ %i*%i*%i with dist = %f\n', length, p, q, s, diff(p,q,s));  
    else
        disp('No solution');
    end
end

