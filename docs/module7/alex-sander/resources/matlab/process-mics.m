function samples = process_mics(x)
    samples = [];
    for i = 1:size(x,2)
        j = 1;
        threshold = max(abs(x(:,i))) / 3;
        while j < length(x) && abs(x(j,i)) < threshold
           j = j + 1;
        end
        samples(i) = j;
    end
end