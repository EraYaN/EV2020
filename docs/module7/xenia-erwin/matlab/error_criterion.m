function [ = error_criterion 


difference = length(h_hat) - length(h);

    if difference > 0
        %h=[h zeros(1,verschil)];
        h = [h , zeros(1,abs(difference))];
    elseif difference < 0
        %h_hat=[h_hat zeros(1,abs(verschil))];
        h_hat = [h_hat ; zeros(abs(difference), 1)];
    end
    
h=h';   
alpha = (hhat' * h)/(hhat' * hhat);
errpr = norm(alpha * hhat - h, 2);

end

